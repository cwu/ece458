#ifndef __SERVER_H__
#define __SERVER_H__

#include <netinet/in.h>

#define PORT_ANY 0

struct server;

struct server* server_create();
void server_destroy(struct server * server);

int server_port(struct server* server);

int server_start(struct server* server, int port);

int server_accept(struct server* server, struct sockaddr_in *client_addr);
void server_detach(struct server* server, int client_fd);
int server_finish(struct server *server, int client_fd);

#endif // __SERVER_H__
