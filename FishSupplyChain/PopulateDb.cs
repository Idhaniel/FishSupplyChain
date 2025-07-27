using FishSupplyChain.Data;
using FishSupplyChain.Entities;
using FishSupplyChain.Services;

namespace FishSupplyChain
{
    public static class PopulateDb
    {
        public static async Task PopulateAsync(FishSupplyChainDbContext dbContext, PasswordHandlerService passwordHandler)
        {
            // Ensure database is created
            await dbContext.Database.EnsureCreatedAsync();

            // Seed initial data
            if (!dbContext.Users.Any())
            {
                var dummyUsers = new List<UserEntity>
            {
                new()
                {
                    FirstName = "Daniel",
                    LastName = "Law",
                    Email = "daniel@gmail.com",
                    Password = passwordHandler.HashPassword("daniel"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x70997970C51812dc3A010C7d01b50e0d17dc79C8",
                        PrivateKey = "0x59c6995e998f97a5a0044966f0945389dc9e86dae88c7a8412f4603b6b78690d",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Benita",
                    LastName = "Scarlet",
                    Email = "benita@gmail.com",
                    Password = passwordHandler.HashPassword("benita"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x3C44CdDdB6a900fa2b585dd299e03d12FA4293BC",
                        PrivateKey = "0x5de4111afa1a4b94908f83103eb1f1706367c2e68ca870fc3fb9a804cdab365a",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Shade",
                    LastName = "Kidd",
                    Email = "shade@gmail.com",
                    Password = passwordHandler.HashPassword("shade"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x90F79bf6EB2c4f870365E785982E1f101E93b906",
                        PrivateKey = "0x7c852118294e51e653712a81e05800f419141751be58f605c371e15141b007a6",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Lumani",
                    LastName = "Lola",
                    Email = "lumani@gmail.com",
                    Password = passwordHandler.HashPassword("lumani"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x15d34AAf54267DB7D7c367839AAf71A00a2C6A65",
                        PrivateKey = "0x47e179ec197488593b187f80a00eb0da91f1b9d0b13f8733639f19c30a34926a",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Moyin",
                    LastName = "Shabi",
                    Email = "moyin@gmail.com",
                    Password = passwordHandler.HashPassword("moyin"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x9965507D1a55bcC2695C58ba16FB37d819B0A4dc",
                        PrivateKey = "0x8b3a350cf5c34c9194ca85829a2df0ec3153be0318b5e2d3348e872092edffba",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Orobosa",
                    LastName = "Stephen",
                    Email = "orobosa@gmail.com",
                    Password = passwordHandler.HashPassword("orobosa"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x976EA74026E726554dB657fA54763abd0C3a0aa9",
                        PrivateKey = "0x92db14e403b83dfe3df233f83dfa3a0d7096f21ca9b0d6d6b8d88b2b4ec1564e",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Fidelis",
                    LastName = "Emmanuel",
                    Email = "fidelis@gmail.com",
                    Password = passwordHandler.HashPassword("fidelis"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x14dC79964da2C08b23698B3D3cc7Ca32193d9955",
                        PrivateKey = "0x4bbbf85ce3377467afe5d46f804f221813b2bb87f24d81f60f1fcdbf7cbf4356",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Dupe",
                    LastName = "Rotimi",
                    Email = "dupe@gmail.com",
                    Password = passwordHandler.HashPassword("dupe"),
                    Wallet = new WalletEntity
                    {
                        Address = "0x23618e81E3f5cdF7f54C3d65f7FBc0aBf5B21E8f",
                        PrivateKey = "0xdbda1821b80551c9d65939329250298aa3472ba22feea921c0cf5d620ea67b97",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Shirley",
                    LastName = "Quixote",
                    Email = "shirley@gmail.com",
                    Password = passwordHandler.HashPassword("shirley"),
                    Wallet = new WalletEntity
                    {
                        Address = "0xa0Ee7A142d267C1f36714E4a8F75612F20a79720",
                        PrivateKey = "0x2a871d0798f97d79848a013d4936a73bf4cc922c825d33c1cf7073dff6d409c6",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },
                new()
                {
                    FirstName = "Lelouch",
                    LastName = "Lamperouge",
                    Email = "lelouch@gmail.com",
                    Password = passwordHandler.HashPassword("lelouch"),
                    Wallet = new WalletEntity
                    {
                        Address = "0xBcd4042DE499D14e55001CcbB24a551F3b954096",
                        PrivateKey = "0xf214f2b2cd398c806f84e317254e0f0b801d0643303237d97a22a48e01628897",
                        SeedPhrase = "test test test test test test test test test test test junk"
                    }
                },

            };

                dbContext.Users.AddRange(dummyUsers);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
