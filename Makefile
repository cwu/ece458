all: exploit stack

run: all
	./exploit
	./stack

exploit: exploit.c
	gcc -o $@ $<
stack: stack.c
	gcc -o $@ $<
