using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;

using LMS.Core.Interfaces;
using LMS.App.ViewModels;

namespace LMS.App.ViewModelToEntityMappings
{
    public class ViewModelToEntityMapping<TViewModel, TEntity> : IViewModelToEntityMapping<TViewModel> where TEntity : IEntity, new()
                                                               where TViewModel : CrudViewModel, new ()
    {
        virtual public IReadOnlyCollection<TViewModel> List(IMapper mapper, IRepository repo, Expression<Predicate<TViewModel>> predicate)
        {
            Expression<Predicate<TEntity>> exp = null;

            if(predicate != null)
                exp = mapper.MapExpression<Expression<Predicate<TEntity>>>(predicate);
            
            return repo.List<TEntity>(exp).Select(x => mapper.Map<TEntity, TViewModel>(x)).ToList(); 
        }

        virtual public int Create(IMapper mapper, IRepository repo, TViewModel vm)
        {
            return repo.Add<TEntity>(mapper.Map<TViewModel, TEntity>(vm));
            
        }

        virtual public void Update(IMapper mapper, IRepository repo, TViewModel vm)
        {
            repo.Update(mapper.Map<TViewModel, TEntity>(vm));
        }

        virtual public void Delete(IMapper mapper, IRepository repo, int id)
        {
            repo.Delete<TEntity>(id);
        }
        
        public virtual IEnumerable<Profile> AutoMappersProfiles => new Profile[] { new Entity2ModelView(), new ModelView2Entity() };
        
        class Entity2ModelView : Profile
        {
            public Entity2ModelView()
            {
                CreateMap<TEntity, TViewModel>();
            }
        }

        class ModelView2Entity : Profile
        {
            public ModelView2Entity()
            {
                CreateMap<TViewModel, TEntity>();
            }
        }
    }
}
