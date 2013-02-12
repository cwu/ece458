all: exploit stack

run: all
	./exploit
	./stack

exploit: exploit.c
	gcc -o $@ $<
stack: stack.c
	gcc -g -fno-stack-protector -o $@ $<
	sudo su -c "chmod 4755 $@"

call_shellcode: call_shellcode.c
	gcc -o $@ $<

clean:
	rm -f *.o exploit stack call_shellcode
