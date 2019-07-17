### csharp代码生成命令行
protoc.exe -I=. --csharp_out=. --grpc_out=. --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe user.proto  
* -I 指定一个或者多个目录，用来搜索.proto文件的，如果不指定，那就是当前目录，因为-I已经指定了。
* --csharp_out 用来生成C#代码，当然了还能cpp_out、java_out、javanano_out、js_out、objc_out、php_out、python_out、ruby_out 这时候你就应该知道，这玩意就是支持多语言的，才用的，生成一些文件，然后给各个语言平台调用。参数1是输出路径，参数2是proto的文件名或者路径。  
* --grpc_out 到这里可能有人会懵逼，咋回事？C#不是有一个自己的输出目录么？怎么又一个输出？csharp_out是输出类似于咱们平时写的实体类，接口，定义之类的。生成的文件叫，额，就叫xxx.cs吧.grpc_out是跟服务相关，创建，调用，绑定，实现相关。生成的玩意叫xxxGrpc.cs。 对比上个选项生成的文件名，大概能了解个十之八九吧。
* --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe 这个就是c#的插件，python有python的，java有java的。必须要指定它。
