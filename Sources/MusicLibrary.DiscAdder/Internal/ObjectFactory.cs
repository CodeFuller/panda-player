﻿using System;
using CF.Library.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MusicLibrary.DiscAdder.Internal
{
	internal class ObjectFactory<TType> : IObjectFactory<TType>
		where TType : class
	{
		private readonly IServiceProvider serviceProvider;

		public ObjectFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public TType CreateInstance()
		{
			return serviceProvider.GetRequiredService<TType>();
		}
	}
}