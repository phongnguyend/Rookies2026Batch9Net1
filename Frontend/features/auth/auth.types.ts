import { UserRoles } from "@/features/users/users.types";

export namespace Login {
    export interface Request {
        username: string;
        password: string;
    }

    // return only cookie, no tokens returns
    export interface Response { };
}

export namespace Refresh {
    export interface Request { }
    export interface Response { }
}

export namespace GetMe {
    export interface Refresh { };
    export interface Response {
        id: string;
        userName: string;
        locationId: string;
        isFirstLogin: boolean;
        roles: string[];
    }
}

export namespace FirstChangePassword {
    export interface Request {
        newPassword: string;
    }
    export interface Response { }
}