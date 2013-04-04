import morph_socket
import socket

if __name__ == '__main__':
  sock = morph_socket.MorphSocket(socket.AF_INET, socket.SOCK_STREAM)
  sock.connect(('localhost', 8081))
  sock.send("x" * 100)
  sock.close()
