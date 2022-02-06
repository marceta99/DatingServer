using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file); //ovo IForm file predstavlja fajl odnosno sliku koju saljemo sa 
                                                               //http requestom .
        Task<DeletionResult> DeletePhotoAsync(string publicId); //svaka slika koja se sacuva na cloudinary dobice svoj publicID
                                                                //i mi cemo koristiti taj publicId da nadjemo tu sliku i obrisemo
    
        
    
    
    
    
    
    }                       
}
