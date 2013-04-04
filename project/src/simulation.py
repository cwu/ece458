import morpheus
import random
import time
import socket
import morph_socket
import sys

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
  if len(sys.argv) < 3:
    print "simulation.py <sim distr path> <target_distr_path>"
    return

  distr_name = sys.argv[1]
  sim_distr = [float(prob) for prob in morpheus.get_distr_from_file(distr_name)]
  target_distr = [float(prob) for prob in morpheus.get_distr_from_file(sys.argv[2])]

  sock = morph_socket.MorphSocket(socket.AF_INET, socket.SOCK_STREAM, target_distr)
  sock.connect(('localhost', 8081))
  last_load = time.time()

  while True:
    size = get_rand_size(sim_distr)
    print 'from %d' % size
    sock.send("x" * size)
    time.sleep(0.01)

    if time.time() - last_load >= 1:
      sim_distr = [float(prob) for prob in morpheus.get_distr_from_file(distr_name)]
      last_load = time.time()

if __name__ == '__main__':
  main()
