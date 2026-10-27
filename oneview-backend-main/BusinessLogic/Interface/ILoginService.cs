using DTO.Authentication;

namespace BusinessLogic.Interface {

    public interface ILoginService {
        DTOLoginResponse Authenticate(DTOLoginRequest request);
    }

}