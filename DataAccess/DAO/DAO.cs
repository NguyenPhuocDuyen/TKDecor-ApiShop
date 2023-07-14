using BusinessObject;

namespace DataAccess.DAO
{
    internal class DAO<T>
    {
        internal static async Task Add(T entity)
        {
            try
            {
                using var context = new TkdecorContext();
                if (entity != null)
                {
                    await context.AddAsync(entity);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        internal static async Task Update(T entity)
        {
            try
            {
                using var context = new TkdecorContext();
                if (entity != null)
                {
                    context.Update(entity);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
