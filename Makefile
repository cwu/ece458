all: exploit stack shell
	./exploit
	- ./stack
shell: shellcode.c
	sudo su -c "gcc -g -static -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"

exploit: exploit.c
	gcc -o $@ $<

stack: stack.c
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"

shh: asm.c
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"
	./shh

call_shellcode: call_shellcode.c
	gcc -o $@ $<
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"
	./$@

clean:
	rm -f *.o exploit stack call_shellcode shell shh
