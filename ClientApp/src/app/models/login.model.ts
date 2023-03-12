export class AuthenticateModel {
    public token: string;
}

export class GetRoleIdModel {
    public roleId: number;
}

export class RenewTokenModel {
    public token: string;
}

export class ValidateTokenModel {
    public result: boolean;
}