class Parser
  def initialize(infile)
    lines = File.readlines(infile)
    lines.map!{ |l| l.encode!('UTF-8', 'UTF-8', :invalid => :replace) }
    @sizes = parseSizes(lines)
  end

  def parseSizes(lines)
    lines.map { |l|
      m = /length (\d+)/.match(l)
      m.nil? ? '0' : m[1]
    }
  end

  def write(outfile)
    File.open(outfile, 'w') { |f|
      @sizes.each { |l| f.puts(l + ',') }
    }
  end
end

Parser.new(ARGV[0]).write(ARGV[1])
