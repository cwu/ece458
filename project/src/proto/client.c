#include "client.h"

#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <netdb.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <sys/socket.h>

int client_connect(const char * hostname, const char * port) {
  struct addrinfo hints, *servinfo, *p;

  memset(&hints, 0, sizeof hints);
  hints.ai_family = AF_INET;
  hints.ai_socktype = SOCK_STREAM;

  int ret = getaddrinfo(hostname, port, &hints, &servinfo);
  if (ret < 0) {
    return -1;
  }

  // loop through all the results and connect to the first we can
  for(p = servinfo; p != NULL; p = p->ai_next) {
    int sock_fd = socket(p->ai_family, p->ai_socktype, p->ai_protocol);
    if (sock_fd < 0) {
      continue;
    }

    int ret = connect(sock_fd, p->ai_addr, p->ai_addrlen);
    if (ret == -1) {
      close(sock_fd);
      continue;
    }
    freeaddrinfo(servinfo);
    return sock_fd;
  }
  return -1;
}

int client_disconnect(int client_fd) {
  return close(client_fd);
}
