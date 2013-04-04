import morpheus
import random
import time
import socket
import morph_socket

def get_rand_size(distr):
  r = random.random()
  cummulative = 0
  size = 0
  for p in distr:
    cummulative += p
    if r < cummulative:
      return size
    size += 1
  raise ValueError("bad random with distribution: %f" % r)

def main():
  sim_distr = [float(prob) for prob in morpheus.get_distr_from_file("data/https_cs_distr.txt")]

  sock = morph_socket.MorphSocket(socket.AF_INET, socket.SOCK_STREAM)
  sock.connect(('localhost', 8081))
  while True:
    size = get_rand_size(sim_distr)
    print 'from %d' % size
    sock.send("x" * size)
    time.sleep(0.1)

if __name__ == '__main__':
  main()
