# Snap

Snap is service, which provide abstractions to manage cached information.

# Example

```c#
using System;
using System.IO;
using System.Net;
using System.Text;
using Snap.Remoting;
using Snap.Abstractions;
using System.Text.Json.Serialization;

//Implementing caching system with API
namespace SnapAbstractionsExample
{
    public abstract class SpaceProvider : ISpaceProvider<ExampleObject[]>
    {
        private string readonly _path;
    
        public SpaceProvider(string path)
        {
            _path = path;
        }
    
       //This method must write ALL data
        public void WriteData(ExampleObject[] data)
        {
            using (var fs = new FileStream(_path, FileMode.Open))
            {
                var json = JsonSerializer.Serialize(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                
                fs.Write(bytes, 0, bytes.Length);
            }                
        }
        
        public ExampleObject[]? GetData()
        {
            using (var fs = new FileStream(_path, FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                
                fs.Read(buffer, 0, buffer.Length);
                
                var json = Encoding.UTF8.GetString(buffer);
                
                try
                {
                    return JsonSerializer.Deserialize<ExampleObject[]>(json); 
                }
                catch(Exception)
                {
                    return null;
                }
                catch(SystemException)
                {
                    return null;
                }
            }
        }
        
        public void ClearData()
        {
            File.WriteAllText(_path, string.Empty);        
        } 
    }

    public class CachedDataFileProvider : SpaceProvider
    {
        public CachedDataFileProvider() : base("cached.json")
        {
        
        }
    }
    
    public class DeltasDataFileProvider : SpaceProvider
    {
        public CachedDataFileProvider() : base("deltas.json")
        {
            
        }    
    }
    
    public class ApiServerProvider : IRemoteServerProvider<ExampleObject[]>
    {
        private readonly HttpClient _client = new HttpClient()
        {
            BaseAddress = "https://example.com"
        };                
    
        public SnapRemoteResult<ExampleObject[]> SendData(ExampleObject obj)
        {
            var json = JsonSerializer.Serialize(obj);
            
            var content = new StringContent(json);
            
            var result = _client.Post("/api/post", content);
            
            if(result.StatusCode != 200) 
            {            
                return new SnapRemoteResult<ExampleObject[]>()
                {
                    Ok = false                
                };
            }
            
            return new SnapRemoteResult<ExampleObject[]>()
            {
                Ok = true,
                Object = JsonSerializer.Deserialize<ExampleObject>(result.ReadAsString())                
            }; 
        }
        
        public SnapRemoteResult<ExampleObject[]> GetData()
        {                       
            var result = _client.Post("/api/get", json);
            
            if(result.StatusCode != 200) 
            {            
                return new SnapRemoteResult<ExampleObject[]>()
                {
                    Ok = false                
                };
            }
            
            return new SnapRemoteResult<ExampleObject[]>()
            {
                Ok = true,
                Object = JsonSerializer.Deserialize<ExampleObject[]>(result.ReadAsString())                
            }; 
        }
    }
    
    public class ExampleObject 
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }
}
```

```c#
public class Program
{
    private readonly SnapServer<ExampleObject> _server;

    public static void Main(string[] args)
    {
        var snapConfig = new SnapConfiguration<ExampleObject[]>()
        {
            DeltasSpaceProvider = new DeltasDataFileProvider(),
            CacheSpaceProvider = new CachedDataFileProvider(),
            RemoteServerProvider = new ApiServerProvider()
        };
        
        _server = snapConfig.ConfigureServer();
    }
}
```

```c#
//....CODE....
var data = _server.GetData();
//Manipulation with this data

//Then send
_server.SendData(data);
```
