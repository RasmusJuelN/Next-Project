using Database.Interfaces;
using Database.Models;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class ActiveQuestionnaireRepository(Context context, ILoggerFactory loggerFactory) : GenericRepository<ActiveQuestionnaireModel>(context, loggerFactory), IActiveQuestionnaireRepository
{
    private readonly Context _context = context;
}
