all: exploit stack shell
	./exploit
	- ./stack

report.pdf: report.pandoc
	pandoc -V geometry:margin=1in -o $@ $<

shell: shellcode.c
	sudo su -c "gcc -g -static -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"

exploit: exploit.c
	gcc -o $@ $<

stack: stack.c
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"

shellcode_asm: shellcode_asm.c
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"
	./$@

call_shellcode: call_shellcode.c
	gcc -o $@ $<
	sudo su -c "gcc -g -fno-stack-protector -o $@ $<"
	sudo su -c "chmod 4755 $@"
	./$@

clean:
	rm -f *.o exploit stack call_shellcode shell shellcode_asm
