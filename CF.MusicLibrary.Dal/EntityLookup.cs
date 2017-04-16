using System;
using System.Data.Entity;
using System.Linq;

namespace CF.MusicLibrary.Dal
{
	/// <summary>
	/// Extension class for entity lookup in DBContext.
	/// </summary>
	public static class EntityLookup
	{
		/// <summary>
		/// Provides entity either by returning existing one in DbSet or by creating the new one by given entity factory.
		/// </summary>
		/// <returns></returns>
		public static TEntity ProvideEntity<TEntity>(this DbSet<TEntity> entitySet, Func<TEntity, bool> comparer, Func<TEntity> entityFactory)
			where TEntity : class
		{
			if (entitySet == null)
			{
				throw new ArgumentNullException(nameof(entitySet));
			}
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}
			if (entityFactory == null)
			{
				throw new ArgumentNullException(nameof(entityFactory));
			}

			TEntity entity = entitySet.Local.SingleOrDefault(comparer);
			if (entity == null)
			{
				entity = entityFactory();
				entitySet.Add(entity);
			}

			return entity;
		}
	}
}
