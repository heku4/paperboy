package main

import (
	"bytes"
	"encoding/hex"
	"fmt"
	"io"
	"log"
	"net/http"
	"os"

	"github.com/psanford/lencode"
	"golang.org/x/net/http2"
	"google.golang.org/protobuf/encoding/protojson"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/reflect/protodesc"
	"google.golang.org/protobuf/reflect/protoregistry"
	"google.golang.org/protobuf/types/descriptorpb"
	"google.golang.org/protobuf/types/known/anypb"
)

func main() {

	protoFile, err := os.ReadFile("/Users/xek/dev/GoProjects/paperboy/src/helloworld.proto")
	if err != nil {
		panic(err)
	}

	fileDescriptors := &descriptorpb.FileDescriptorSet{}
	//parse proto and map to collection
	err = proto.Unmarshal(protoFile, fileDescriptors)
	if err != nil {
		panic(err)
	}

	// get first proto file
	pb := fileDescriptors.GetFile()[0]
	fd, err := protodesc.NewFile(pb, protoregistry.GlobalFiles)
	if err != nil {
		panic(err)
	}

	// registration
	err = protoregistry.GlobalFiles.RegisterFile(fd)
	if err != nil {
		panic(err)
	}

	// message creation
	var out bytes.Buffer
	requestMessageType, err := protoregistry.GlobalTypes.FindMessageByName("helloworld.HelloRequest")
	if err != nil {
		panic(err)
	}
	enc := lencode.NewEncoder(&out, lencode.SeparatorOpt([]byte{0}))

	// jsonlike default message
	j := `{	"@type": "helloworld.HelloRequest", "name": "officia consectetur"}`
	requestBodyByteInstance, err := anypb.New(requestMessageType.New().Interface())
	if err != nil {
		panic(err)
	}

	// write bytes from j to body
	err = protojson.Unmarshal([]byte(j), requestBodyByteInstance)
	if err != nil {
		panic(err)
	}
	fmt.Printf("Encoded HelloRequest using protojson and anypb %v\n", hex.EncodeToString(requestBodyByteInstance.Value))

	// to send the json->protobuf message
	err = enc.Encode(requestBodyByteInstance.Value)
	if err != nil {
		panic(err)
	}
	reader := bytes.NewReader(out.Bytes())

	// create client and send request
	client := http.Client{
		Transport: &http2.Transport{},
	}

	resp, err := client.Post("https://localhost:50051/helloworld.Greeter/SayHello", "application/grpc", reader)
	if resp.StatusCode != http.StatusOK {
		log.Fatal(err)
	}

	// get bytes from response body and decode it
	bodyBytes, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Fatal(err)
	}
	bytesReader := bytes.NewReader(bodyBytes)
	respMessage := lencode.NewDecoder(bytesReader, lencode.SeparatorOpt([]byte{0}))
	respMessageBytes, err := respMessage.Decode()
	if err != nil {
		log.Fatal(err)
	}

	// search registered response type
	responseMessageType, err := protoregistry.GlobalTypes.FindMessageByName("helloworld.HelloReply")
	if err != nil {
		log.Fatal(err)
	}

	// allocate memory for response specific type
	echoReplyMessageDescriptor := responseMessageType.Descriptor()
	pmr := responseMessageType.New()

	// deserialize
	err = proto.Unmarshal(respMessageBytes, pmr.Interface())
	if err != nil {
		log.Fatal(err)
	}

	msg := echoReplyMessageDescriptor.Fields().ByName("message")

	fmt.Printf("EchoReply.Message using protoreflect: %s\n", pmr.Get(msg).String())
}
