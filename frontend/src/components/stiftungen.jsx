import { useSession } from "./session_provider";
import { useEffect, useState } from "react";
import { Empty } from "../protobuf-javascript/google/protobuf/empty_pb.js";

export default function Stiftungen() {
    const { grpcClients, isGrpcClientsSet } = useSession();
    const [stiftungen, setStiftungen] = useState([]);

    useEffect(() => {
        console.log("GrpcClients: ", grpcClients);

        const client = grpcClients["navClient"];

        if (!client) {
            return;
        }

        const stream = client.getStiftungenStream(new Empty());
        stream.on("data", response => {
            const navData = response.toObject();
            setStiftungen(prevStiftungen => [...prevStiftungen, navData]);
        });

        return () => stream.cancel();
    }, [isGrpcClientsSet]);

    return (
        <div>
            <h1>Stiftungen</h1>
            <ul>
                {stiftungen.map((stiftung, index) => (
                    <li key={index}>{stiftung.text}</li>
                ))}
            </ul>
        </div>
    );
}
