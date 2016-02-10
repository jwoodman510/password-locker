using System.Web;
using Caching;
using DataAccess;
using DataAccess.Dal;
using Microsoft.Practices.Unity;
using PasswordLocker;
using Unity.WebForms;

[assembly: WebActivatorEx.PostApplicationStartMethod( typeof(UnityWebFormsStart), "PostStart" )]
namespace PasswordLocker
{
	/// <summary>
	///		Startup class for the Unity.WebForms NuGet package.
	/// </summary>
	internal static class UnityWebFormsStart
	{
		/// <summary>
		///     Initializes the unity container when the application starts up.
		/// </summary>
		/// <remarks>
		///		Do not edit this method. Perform any modifications in the
		///		<see cref="RegisterDependencies" /> method.
		/// </remarks>
		internal static void PostStart()
		{
			IUnityContainer container = new UnityContainer();
			HttpContext.Current.Application.SetContainer( container );

			RegisterDependencies( container );
		}

		/// <summary>
		///		Registers dependencies in the supplied container.
		/// </summary>
		/// <param name="container">Instance of the container to populate.</param>
		private static void RegisterDependencies( IUnityContainer container )
		{
            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<ICompanyDal, CompanyDal>();
            container.RegisterType<ICache, Cache>(new ContainerControlledLifetimeManager());
        }
	}
}