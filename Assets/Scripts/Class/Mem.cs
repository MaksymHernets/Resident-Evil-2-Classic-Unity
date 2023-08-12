using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Mem
{
    public DataView _buf;
    public int _pc;
    public int _ps;
    public Stack<int> _stack;
    public Stack<int> sub_stack;

    public string indentation = "";

    Mem(DataView buf, int pc)
    {
        if (pc == 0) throw new Error("bad sub function number");

        this._buf = buf;
        this._pc = pc;
        this._ps = 0;
        this._stack = new Stack<int>();
        this.sub_stack = new Stack<int>();
    }

    public byte Byte() {
    var d = this._buf.getUint8(this._pc);
    // debug(">", h(this._pc), d);
    this._pc += 1;
    return (byte)d;
  }

public char Char() {
    var d = this._buf.getInt8(this._pc);
    // debug(">", h(this._pc), d);
    this._pc += 1;
    return (char)d;
}

public ushort Ushort () {
    var d = this._buf.getUint16(this._pc, true);
    // debug(">", h(this._pc), d);
    this._pc += 2;
    return (ushort)d;
}

public short Short() {
    var d = this._buf.getInt16(this._pc, true);
    // debug(">", h(this._pc), d);
    this._pc += 2;
    return (short)d;
}

public ulong Ulong () {
    var d = this._buf.getUint32(this._pc, true);
    this._ps += 4;
    return (ulong)d;
}

// 跳过 n 个字节
public void s(int n) {
    // console.debug("^", h(this._pc), "[", 
    //     new Uint8Array(this._buf.buffer, this._pc, n), "]");
    this._pc += n;
}

    // 值压入栈
    public void push(int pc) {
    // debug("PUSH", h(this._pc), h(pc));
    this._stack.Push(pc);
    indentation += "  ";
    // TODO: 过多的缩进是 bug
    if (indentation.Length > 20)
    {
        throw new Error("many indentation");
    }
}

    // 弹出的栈值作为 pc 的值
    public int pop() {
    if (this._stack.Count <= 0)
        throw new Error("POP empty stack");
    var v = this._stack.Pop();
    // debug("POP", h(v));
    indentation = indentation.Substring(2);
    return v;
}

    // 弹出的栈值作为 pc 的值
    public void poppc() {
    this._pc = this.pop();
}

    // 返回栈顶的值但不弹出, i 是栈顶向栈底的偏移
    public int top(int i) {
    if (this._stack.Count <= 0)
    {
        throw new Error("TOP empty stack");
    }
    int off = this._stack.Count - 1 - i;
    if (off < 0)
    {
        throw new Error("stack index out bound");
    }
    return this._stack.ToArray()[off];
}

    public bool is_stack_non() {
    return this._stack.Count == 0;
}
}

