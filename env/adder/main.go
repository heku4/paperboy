package main

import (
	"context"
	"flag"
	"fmt"
	"log"
	"net"

	adder "adder/api/proto"

	"google.golang.org/grpc"
)

var (
	port = flag.Int("port", 50051, "The server port")
)

type server struct {
	adder.UnimplementedAdderServer
}

func (s *server) Add(_ context.Context, command *adder.AddCommand) (*adder.AddCommandResponse, error) {
	log.Printf("Received: %v,  %v", command.LeftOperand, command.RightOperand)
	return &adder.AddCommandResponse{Result: int64(command.LeftOperand) + int64(command.RightOperand)}, nil
}

func main() {
	flag.Parse()
	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", *port))
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}
	s := grpc.NewServer()
	adder.RegisterAdderServer(s, &server{})
	log.Printf("server listening at %v", lis.Addr())
	if err := s.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}
