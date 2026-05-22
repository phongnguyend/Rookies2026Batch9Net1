import { UserRoles } from "@/features/users/users.types";

export namespace Login {
    export interface Request {
        username: string;
        password: string;
    }

    // return only cookie, no tokens returns
    export interface Response { };
}

export namespace GetMe {
    export interface Response {
        username: string;
        role: UserRoles;
        isFirstLogin: boolean;
    }
}