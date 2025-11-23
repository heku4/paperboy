# Paperboy

## Goals

- Create GRPC calls-collections to run in various test-scenrios.
- Decorate each call or collecction part by specific operations:
  - Call times
  - Calls until specific result / fail
  - Parallel calls
- Link reesponse from previous call with next request via variables.
- Customize headers
- Create UI

## Dev notes

### Usefull links

- [Crafting Raw gRPC Request Without Protobuf File — HackToday 2020 Write Up](https://medium.com/@idzharbae/crafting-raw-grpc-request-without-protobuf-file-hacktoday-2020-write-up-ea6ef337d365)
- [Nice demo project - protoreflect](https://github.com/jhump/protoreflect)

### about .proto

This example is valid for `adder` project from `env` directory.

- `package` contains package name `adder;`
- `go_package` contains reelative directory path from project `option go_package="./api";`

### protoc

Create '\$api\$' directory. Create `.proto` file in this directory. Run command:

```bash
protoc --go_out=. --go_opt=paths=source_relative --go-grpc_out=. --go-grpc_opt=paths=source_relative PROTO_FILE_NAME.proto
```

[Protoc quick-start-guide](https://grpc.io/docs/languages/go/quickstart/) - install and use examples
