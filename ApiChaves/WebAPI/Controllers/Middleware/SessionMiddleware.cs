namespace ApiChaves.WebAPI.Controllers.Middleware
{
    public class SessionMiddlleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddlleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifique se há uma sessão ativa
            if (context.Session.GetString("Id") == null || context.Session.GetString("Usuario") == null)
            {
                // Se não houver sessão ativa, redirecione para a página de login ou retorne um erro
                context.Response.Redirect("Login/autenticar"); // Altere para o caminho correto
                return;
            }

            await _next(context);
        }
    }

}
