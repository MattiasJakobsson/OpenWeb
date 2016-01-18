﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using SuperGlue.Configuration;
using SuperGlue.EventStore.ConflictManagement;
using SuperGlue.EventStore.Timeouts;

namespace SuperGlue.EventStore.Data
{
    public class ConfigureEventStore : ISetupConfigurations
    {
        private static EventStoreConnectionString connectionString;
        private static IEventStoreConnection connection;

        public IEnumerable<ConfigurationSetupResult> Setup(string applicationEnvironment)
        {
            yield return new ConfigurationSetupResult("superglue.EventStore.Setup", environment =>
            {
                environment.AlterSettings<EventStoreSettings>(x =>
                {
                    x.ModifySettings(y =>
                    {
                        y.KeepReconnecting();
                        y.KeepRetrying();
                        y.UseCustomLogger(new EventStoreLog(environment));
                    });

                    x.StoreCommands((env, command, id, causationId) => "commands");
                });

                environment.RegisterSingleton(typeof(IEventStoreConnection), (x, y) => connection);
                environment.RegisterSingleton(typeof(EventStoreConnectionString), (x, y) => connectionString);
                environment.RegisterAll(typeof(IManageChanges));
                environment.RegisterTransient(typeof(IHandleEventSerialization), typeof(DefaultEventSerializer));
                environment.RegisterTransient(typeof(IRepository), typeof(DefaultRepository));
                environment.RegisterTransient(typeof(ICheckConflicts), typeof(DefaultConflictChecker));
                environment.RegisterAllClosing(typeof(ICheckConflict<,>));
                environment.RegisterTransient(typeof(IManageTimeOuts), typeof(DefaultTimeOutManager));

                TimeOutManager.Configure(() => new StoreTimeoutsInMemory());

                return Task.CompletedTask;
            }, "superglue.LoggingSetup", environment =>
            {
                connection.Close();

                return Task.CompletedTask;
            }, configuration =>
            {
                var connectionInformation = configuration
                    .WithSettings<EventStoreSettings>()
                    .CreateConnection(configuration.Settings.Resolve<IApplicationConfiguration>());

                connection = connectionInformation.Item2;
                connectionString = connectionInformation.Item1;

                return connection.ConnectAsync();
            });
        }
    }
}