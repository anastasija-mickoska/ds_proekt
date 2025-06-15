using ds_proekt.Models;
public class ParfumeDetailsViewModel
{
    public Parfume Parfume { get; set; }
    public List<ReviewWithEmail> Reviews { get; set; }
    
}
public class ReviewWithEmail
{
    public Review Review { get; set; }
    public string UserEmail { get; set; }
}
