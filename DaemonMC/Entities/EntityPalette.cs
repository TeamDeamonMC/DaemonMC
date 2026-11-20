using System.Reflection;
using DaemonMC.Utils.Text;

namespace DaemonMC.Entities
{
    public class EntityPalette
    {
        private static readonly Dictionary<string, Func<Entity>> entityFactories = new Dictionary<string, Func<Entity>>(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlyCollection<string> Identifiers => entityFactories.Keys;

        public static void buildPalette()
        {
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Entity)) && t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var type in entityTypes)
            {
                Entity entity = (Entity)Activator.CreateInstance(type)!;

                if (string.IsNullOrEmpty(entity.ActorType))
                {
                    continue;
                }

                entityFactories.TryAdd(entity.ActorType, () => (Entity)Activator.CreateInstance(type)!);
            }

            Log.debug($"Registered {entityFactories.Count} entity types");
        }

        public static void Register(string identifier, Func<Entity> factory)
        {
            entityFactories[identifier] = factory;
        }

        public static void Register<T>(string identifier) where T : Entity, new()
        {
            Register(identifier, () => new T());
        }

        public static bool IsRegistered(string identifier)
        {
            return entityFactories.ContainsKey(identifier);
        }

        public static Entity Create(string identifier)
        {
            if (entityFactories.TryGetValue(identifier, out var factory))
            {
                return factory();
            }

            return new GenericEntity(identifier);
        }
    }
}
