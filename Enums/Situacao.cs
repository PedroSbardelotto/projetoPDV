using System.ComponentModel;

namespace PDV.Enums
{
    public enum Situacao
    {
        [Description("Finalizado")]
        Finalizado = 1,
        [Description("Pendente")]
        Pendente = 2,
    }
}
