public sealed class Config {
    public required string GoogleApiClientId { get; set; } = null!;

    public static Config Read(string file) {
        IConfigurationRoot configRoot = new ConfigurationBuilder().AddJsonFile(file)
                                                                  .AddEnvironmentVariables()
                                                                  .Build();

        Config? config = configRoot.GetRequiredSection("Config").Get<Config>();

        if (config == null)
            throw new InvalidDataException($"Section \"Config\" not defined in {file}");

        return config;
    }
}

