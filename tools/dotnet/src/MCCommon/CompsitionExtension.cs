using System.Composition.Hosting;
using System.Reflection;

namespace MCCommon
{
    public static class CompsitionExtension
    {
        public static ContainerConfiguration AddModelCoordinationCommon(this ContainerConfiguration configuraiton)
        {
            return configuraiton.WithAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
