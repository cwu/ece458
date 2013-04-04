require 'date'

class Parser
  def initialize(filename)
    `tcpdump -r #{filename}.dump > #{filename}.txt`
    lines = File.readlines(filename + ".txt")
    lines.map!{ |l| l.encode!('UTF-8', 'UTF-8', :invalid => :replace) }
    @sizes = parseSizes(lines)
    @times = parseTimes(lines)
  end

  def parseSizes(lines)
    lines.map { |l|
      m = /length (\d+)/.match(l)
      m.nil? ? '0' : m[1]
    }
  end

  def parseTimes(lines)
    re = /\d{2}:\d{2}:(\d{2}.\d{6})/
    lastTime = re.match(lines[0])[1].to_f
    times = []
    lines.each do |l|
      time = re.match(l)
      next if time.nil?
      time = time[1].to_f
      times << time - lastTime
      lastTime = time
    end
    times
  end

  def writeSizesDistribution(filename)
    distrib = {}
    total = 0
    max = 0
    @sizes.each { |s|
      distrib[s.to_i].nil? ? distrib[s.to_i] = 0 : distrib[s.to_i] += 1
      total += 1
      max = [max, s.to_i].max
    }
    distrib.merge!(distrib) { |k,v| v.to_f/total.to_f }

    File.open(filename + "_dist.txt", 'w') { |f|
      f.puts("# Packet length probability distribution")
      (1..max).each { |i|
        num = distrib[i].nil? ? "0" : distrib[i].to_s
        f.puts(i.to_s + ": " + num)
      }
    }
  end

  def writeSizesCSV(filename)
    File.open(filename + ".csv", 'w') { |f|
      f.puts("Size,")
      @sizes.each { |l| f.puts(l + ',') }
    }
  end

  def writeTimesCSV(filename)
    File.open(filename + "_times.csv", 'w') { |f|
      f.puts("TimeDiff,")
      @times.each { |l| f.puts(l.to_s + ',') }
    }
  end
end

def repeat_every(interval, &block)
  loop do
    start_time = Time.now
    Thread.new(&block).join
    elapsed = Time.now - start_time
    sleep([interval - elapsed, 0].max)
  end
end

repeat_every(2) do
  puts "Parsing tcpdump..."
  p = Parser.new(ARGV[0])
  p.writeSizesCSV(ARGV[0]);
  p.writeSizesDistribution(ARGV[0])
end
