#include "server.h"

#include <stdio.h>
#include <stdlib.h>
#include <strings.h>
#include <string.h>
#include <errno.h>
#include <sys/time.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>

int main() {
  struct server *serv = server_create();
  server_start(serv, 8081);
    struct sockaddr_in addr;
  int client_fd = server_accept(serv, &addr);
  while (1) {
    char buffer[1];
    int n = read(client_fd, buffer, sizeof(buffer));
    if (n >= 0) {
      int i;
      for (i=0;i<n;i++) {
        printf("+");
        fflush(stdout);
      }
    }
  }
  return 0;
}
