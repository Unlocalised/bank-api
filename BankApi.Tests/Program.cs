﻿using Microsoft.AspNetCore.Http.HttpResults;

public class Test
{
    private static BankDb? databaseContext;
    private static string bankId ="29e26195-cf57-417d-ac1b-998398e2dc88";

    [Before(Class)]
    public static Task CreateContext()
    {
        databaseContext = new MockDb().CreateDbContext();
        return Task.CompletedTask;
    }

    [Test]
    public async Task CreateBankReturnsCreated()
    {
        var response = await BankOperation.CreateBank(new BankModel()
        {
            BankTier = BankTier.A,
            Id = Guid.Parse(bankId),
            IsCompliant = false
        }, databaseContext!);

        await Assert.That(response.Result).IsTypeOf<Created<BankModel>>();
    }

    [Test, DependsOn(nameof(CreateBankReturnsCreated))]
    public async Task UpdateBankReturnsNoContent()
    {
        var response = await BankOperation.UpdateBank(Guid.Parse(bankId), new BankModel()
        {
            BankTier = BankTier.A,
            Id = Guid.Parse(bankId),
            IsCompliant = true
        }, databaseContext!);

        await Assert.That(response.Result).IsTypeOf<NoContent>();
    }
}
