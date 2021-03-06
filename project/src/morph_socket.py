import socket
import dream
import time
import random

class MorphSocket(object):
  def __init__(self, family, type_, distr):
    self.socket = socket.socket(family, type_)
    if type_ == socket.SOCK_STREAM:
      self.socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
    start = time.time()
    self.mm = dream.MorphingMatrix(dream.get_csc_from_mm("https_to_tor.mtx"))
    self.dst_distr = distr
    end = time.time()
    print "Dst distribution: sum = %f | len = %d" % (sum(self.dst_distr), len(self.dst_distr))
    print "Time to load morphing matrix : %f" % (end - start)

  def connect(self, host_port):
    self.socket.connect(host_port)

  def get_rand_size(self):
    r = random.random()
    cummulative = 0
    size = 0
    for p in self.dst_distr:
      cummulative += p
      if r < cummulative:
        return size
      size += 1
    return 1460

  def send(self, data):
    if len(data) == 0:
      data = "1"
    size = self.mm.get_target_length(len(data))
    self._send(data, size)

  def _send(self, data, size):
    if len(data) == 0:
      self.socket.send("")
      return

    while len(data) > 0:
      if size > len(data):
        data = data + "-" * (size - len(data))

      self.socket.send(data[:size])
      print ' to %d' % size
      data = data[size:]
      size = self.get_rand_size()

  def close(self):
    self.socket.close()
