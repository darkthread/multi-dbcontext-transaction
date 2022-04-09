using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ext_trans_dbctx
{
    public class TransTester
    {
        private readonly AlphaDbContext alphaCtx;
        private readonly BetaDbContext betaCtx;
        public TransTester(AlphaDbContext alphaCtx, BetaDbContext betaCtx)
        {
            this.alphaCtx = alphaCtx;
            this.betaCtx = betaCtx;
        }
        public void ResetData()
        {
            alphaCtx.Database.Migrate();
            alphaCtx.Alphas.RemoveRange(alphaCtx.Alphas.ToArray());
            alphaCtx.Alphas.Add(new Alpha { Name = "Init" });
            alphaCtx.SaveChanges();

            betaCtx.Database.Migrate();
            betaCtx.Betas.RemoveRange(betaCtx.Betas.ToArray());
            betaCtx.Betas.Add(new Beta { Name = "Init" });
            betaCtx.SaveChanges();
        }

        public string Check()
        {
            return "Alphas: " +
                string.Join(',', alphaCtx.Alphas.Select(o => o.Name).ToArray()) + "\n" +
                "Betas: " +
                string.Join(',', betaCtx.Betas.Select(o => o.Name).ToArray());
        }
        public string TestTranScope()
        {
            ResetData();
            var sb = new StringBuilder();
            sb.AppendLine("*** TestTranScope ***");
            using (var ts = new TransactionScope())
            {
                try
                {
                    alphaCtx.Alphas.Add(new Alpha { Name = "To Rollback" });
                    alphaCtx.SaveChanges();
                    betaCtx.Betas.Add(new Beta { Name = "Duplicated" });
                    betaCtx.Betas.Add(new Beta { Name = "Duplicated" });
                    betaCtx.SaveChanges();
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.Message + "\n" + ex.InnerException?.Message);
                }
            }
            sb.AppendLine(Check());
            return sb.ToString();
        }
        public string TestSharedTrans()
        {
            ResetData();
            var sb = new StringBuilder();
            sb.AppendLine("*** TestSharedTrans ***");
            using (var cn = alphaCtx.Database.GetDbConnection())
            {
                var trn = alphaCtx.Database.BeginTransaction();
                try
                {
                    alphaCtx.Alphas.Add(new Alpha { Name = "To Rollback" });
                    alphaCtx.SaveChanges();
                    // 以現有 DbConnection 建立 BetaDbContext
                    var optBuilder = new DbContextOptionsBuilder<BetaDbContext>();
                    if (alphaCtx.Database.ProviderName.Contains("Sqlite"))
                        optBuilder.UseSqlite(cn);
                    else optBuilder.UseSqlServer(cn);
                    var b = new BetaDbContext(optBuilder.Options);
                    // 呼叫 Datadata.UseTransaction() 參與同一交易
                    b.Database.UseTransaction(trn.GetDbTransaction());
                    b.Betas.Add(new Beta { Name = "Duplicated" });
                    b.Betas.Add(new Beta { Name = "Duplicated" });
                    b.SaveChanges();
                    trn.Commit();
                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.Message + "\n" + ex.InnerException?.Message);
                    trn.Rollback();
                }
                sb.AppendLine(Check());
                return sb.ToString();
            }
        }
    }
}