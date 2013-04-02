require 'date'

class Parser
  def initialize(infile)
    lines = File.readlines(infile)
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
    re = /\d{2}:\d{2}:\d{2}.\d{6}/
    lastTime = DateTime.parse(re.match(lines[0])[0])
    lines.map do |l|
      time = re.match(l)
      next if time.nil?
      time = DateTime.parse(time[0])
      lastTime = time
      time - lastTime
    end
  end

  def write(sizesFile, timesFile)
    File.open(sizesFile, 'w') { |f|
      f.puts("Size,")
      @sizes.each { |l| f.puts(l + ',') }
    }
    File.open(timesFile, 'w') { |f|
      f.puts("TimeDiff,")
      @times.each { |l| f.puts(l.to_s + ',') }
    }
  end
end

Parser.new(ARGV[0]).write(ARGV[1], ARGV[2])
