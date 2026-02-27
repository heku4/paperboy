## Debug http curl script

Run from `tests` directory:

```shell
curl --location 'http://localhost:5049/Debug/ConvertToJsonWithStubs' \                       ✔ 
--header 'Content-Type: multipart/form-data' \     
--form 'File=@./example.proto' -v
```

```shell
curl --location 'http://localhost:5049/Debug/ConvertToJson' \                              ✔ 
--header 'Content-Type: multipart/form-data' \
--form 'File=@./example.proto' -v
```