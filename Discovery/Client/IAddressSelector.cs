namespace Discovery.Client
{
    public interface IAddressSelector
    {
        string Selector(string serviceName);
    }
}