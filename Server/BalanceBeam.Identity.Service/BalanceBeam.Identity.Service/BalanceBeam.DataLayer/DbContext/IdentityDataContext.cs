namespace BalanceBeam.Identity.DataAccess.DbContext
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The identity database context
    /// </summary>
    public class IdentityDataContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        /// <summary>
        /// Constructor for the identity database context
        /// </summary>
        /// <param name="opt">The database context options</param>
        public IdentityDataContext(DbContextOptions<IdentityDataContext> opt) : base(opt)
        {

        }
    }
}
