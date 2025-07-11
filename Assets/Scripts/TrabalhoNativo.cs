using System;
using System.Runtime.InteropServices;
using System.Text;

public static class TrabalhoNativo
{
    [DllImport("trabalho_ed")]
    public static extern void Lista_cria(IntPtr lista);

    [DllImport("trabalho_ed")]
    public static extern bool Lista_inserir(IntPtr lista, string tarefa);

    [DllImport("trabalho_ed")]
    public static extern bool Lista_removerPorNome(IntPtr lista, string nome, StringBuilder buffer);

    [DllImport("trabalho_ed")]
    public static extern void Lista_obterTodas(IntPtr lista, StringBuilder buffer, int bufferSize);


    [DllImport("trabalho_ed")]
    public static extern bool Lista_isEmpty(IntPtr lista);

    [DllImport("trabalho_ed")]
    public static extern void Lista_destruir(IntPtr lista);

    [DllImport("trabalho_ed")]
    public static extern void Lista_mostrar(IntPtr lista, StringBuilder buffer);


    [DllImport("trabalho_ed")]
    public static extern void Fila_create(IntPtr pilha, int sizeElement, int max, int type);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_push(IntPtr pilha, string data);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_pop(IntPtr pilha, StringBuilder buffer);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_isEmpty(IntPtr pilha);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_isFull(IntPtr pilha);

    [DllImport("trabalho_ed")]
    public static extern void Fila_destroy(IntPtr pilha);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_put(IntPtr pilha, string data);

    [DllImport("trabalho_ed")]
    public static extern bool Fila_get(IntPtr pilha, StringBuilder buffer);

}
