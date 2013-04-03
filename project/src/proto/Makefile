
all: test_server test_client

test_server: test_server.c server.c
	gcc -o $@ $^

test_client: test_client.c client.c
	gcc -o $@ $^
