import { createContext, useContext, useState, useEffect } from "react";
import { NavClient } from "../generated/nav_grpc_web_pb";
import { ImageResourceClient } from "../generated/image_resource_grpc_web_pb";

const SessionContext = createContext();

export const useSession = () => useContext(SessionContext);

export const SessionProvider = ({ children, baseAddress }) => {
    const [grpcClients, setGrpcClients] = useState({});
    const [isGrpcClientsSet, setIsGrpcClientsSet] = useState(false);

    useEffect(() => {
        console.log("Base address: ", baseAddress);

        setGrpcClients({
            navClient: new NavClient(baseAddress),
            imageResourceClient: new ImageResourceClient(baseAddress)
        });

        setIsGrpcClientsSet(true);
    }, []);

    return (
        <SessionContext.Provider
            value={{
                grpcClients,
                isGrpcClientsSet
            }}
        >
            {children}
        </SessionContext.Provider>
    );
};
