using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Tutorial3_Task;

   


class EmptyBatteryException : Exception
{
    public EmptyBatteryException() : base("Battery level is too low to turn it on.") { }
}


interface IPowerNotify
{
    void Notify();
}