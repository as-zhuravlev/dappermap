using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using AutoMapper;


using LMS.App.ViewModels;
using LMS.Core.Interfaces;


namespace LMS.App.ViewModelToEntityMappings
{
    public interface IViewModelToEntityMapping<TViewModel> : IViewModelToEntityMappingBase where TViewModel : CrudViewModel
    {
        IReadOnlyCollection<TViewModel> List(IMapper mapper, IRepository repo, Expression<Predicate<TViewModel>> predicate = null);

        int Create(IMapper mapper, IRepository repo, TViewModel vm);
        
        void Update(IMapper mapper, IRepository repo, TViewModel vm);
        
        void Delete(IMapper mapper, IRepository repo, int id);
    }
}
