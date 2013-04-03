#include "server.h"

#include <stdlib.h>
#include <strings.h>
#include <string.h>
#include <errno.h>
#include <sys/time.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>

#define BACKLOG_QUEUE 10

struct server {
  int socket_fd;
  struct sockaddr_in server_addr;
  int max_fd;
  fd_set read_fd_set;
};

static int max(int a, int b) { return a > b ? a : b; }

struct server* server_create() {
  struct server* server = malloc(sizeof(struct server));
  if (server) {
    server->max_fd = 0;
  }
  return server;
}

void server_destroy(struct server * server){
  free(server);
}

int server_port(struct server* server) {
  struct sockaddr addr;
  unsigned int addr_len = sizeof(addr);
  int ret = getsockname(server->socket_fd, &addr, &addr_len);
  if (ret < 0) {
    return -1;
  }

  return ntohs(((struct sockaddr_in*) &addr)->sin_port);
}

int server_start(struct server* server, int port) {
  server->socket_fd = socket(AF_INET, SOCK_STREAM, 0);

  // TODO remove before handin
  int yes = 1;
  setsockopt(server->socket_fd, SOL_SOCKET, SO_REUSEADDR, &yes, sizeof(yes));

  // set the server address
  bzero((char *) &server->server_addr, sizeof(server->server_addr));
  server->server_addr.sin_family = AF_INET;
  server->server_addr.sin_port = htons(port);
  server->server_addr.sin_addr.s_addr = INADDR_ANY;

  // bind the socket
  int ret = bind(server->socket_fd, (struct sockaddr *) &server->server_addr, sizeof(server->server_addr));

  if (ret < 0) {
    return -1;
  }

  ret = listen(server->socket_fd, BACKLOG_QUEUE);
  if (ret < 0) {
    return -1;
  }

  // add the socket_fd into the read set
  FD_ZERO(&server->read_fd_set);
  FD_SET(server->socket_fd, &server->read_fd_set);
  server->max_fd = server->socket_fd;

  return 0;
}

int server_accept(struct server* server, struct sockaddr_in *client_addr) {
  while (1) {
    fd_set read_set = server->read_fd_set;
    int ret = select(server->max_fd + 1, &read_set, NULL, NULL, NULL);
    if (ret < 0) {
      return -1;
    }

    int fd;
    for (fd = 0; fd <= server->max_fd; fd++) {
      if (FD_ISSET(fd, &read_set)) {
        if (fd == server->socket_fd) {
          unsigned int client_length = sizeof(*client_addr);
          int client_fd = accept(server->socket_fd, (struct sockaddr *)client_addr, &client_length);
          if (client_fd < 0) {
          }
          FD_SET(client_fd, &server->read_fd_set);
          server->max_fd = max(server->max_fd, client_fd);
        } else {
          return fd;
        }
      }
    }
  }
}

void server_detach(struct server *server, int client_fd) {
  FD_CLR(client_fd, &server->read_fd_set);
}

int server_finish(struct server *server, int client_fd) {
  int ret = close(client_fd);
  server_detach(server, client_fd);
  return ret;
}
