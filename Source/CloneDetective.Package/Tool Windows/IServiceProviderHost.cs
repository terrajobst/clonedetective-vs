using System;

namespace CloneDetective.Package
{
	public interface IServiceProviderHost
	{
		void Initialize(IServiceProvider serviceProvider);
	}
}