#!/usr/bin/env bash

curl --location 'http://localhost:5049/Debug/ConvertToJsonWithStubsDynamic' --header 'Content-Type: multipart/form-data' --form 'File=@./example.proto' -v
