#include "client.h"

#include <stdio.h>
#include <stdlib.h>
#include <strings.h>
#include <string.h>
#include <errno.h>
#include <sys/time.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>

int main(int argc, char**argv) {
  int fd = client_connect("localhost", "8081");
  int i;
  char buffer[1024];
  for(i=0;i<1024;i++) {
    buffer[i] = '1';
  }
  int n = atoi(argv[1]);
  printf("n : %d\n", n);
  for (i=0;i<200;i++) {
    send(fd, buffer, 2, 0);
  }
  close(fd);
  return 0;
}
