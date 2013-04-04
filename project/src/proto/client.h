#ifndef __CLIENT_H__
#define __CLIENT_H__

int client_connect(const char * hostname, const char * port);
int client_disconnect(int client_fd);

#endif // __CLIENT_H__
